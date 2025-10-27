"""
Enhanced Scoring Engine with Multiple Matching Algorithms
Includes: Keyword matching, semantic similarity, skill taxonomy, experience weighting
"""

from typing import Dict, List, Tuple
from dataclasses import dataclass
import numpy as np
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity
from sentence_transformers import SentenceTransformer
import re

@dataclass
class MatchScore:
    """Comprehensive match score breakdown"""
    overall_score: float
    skill_match_score: float
    experience_match_score: float
    education_match_score: float
    semantic_similarity: float
    keyword_density: float
    missing_critical_skills: List[str]
    matched_skills: List[str]
    preferred_skills_matched: List[str]
    recommendation: str
    confidence_level: str

class EnhancedScoringEngine:
    """Advanced resume scoring with multiple algorithms"""
    
    def __init__(self, use_transformers: bool = False):
        self.use_transformers = use_transformers
        
        # Load semantic model if requested
        if use_transformers:
            try:
                self.semantic_model = SentenceTransformer('all-MiniLM-L6-v2')
            except:
                print("Sentence transformers not available, using TF-IDF")
                self.semantic_model = None
        else:
            self.semantic_model = None
        
        # Skill taxonomy for hierarchical matching
        self.skill_taxonomy = self._build_skill_taxonomy()
        
        # Experience level mapping
        self.experience_weights = {
            'intern': 1,
            'junior': 2,
            'mid-level': 3,
            'senior': 4,
            'lead': 5,
            'principal': 6,
            'staff': 6
        }
    
    def _build_skill_taxonomy(self) -> Dict[str, List[str]]:
        """Build skill hierarchy for related skill matching"""
        return {
            'python': ['pandas', 'numpy', 'scikit-learn', 'django', 'flask'],
            'machine learning': ['deep learning', 'neural networks', 'tensorflow', 'pytorch'],
            'data science': ['machine learning', 'statistics', 'data analysis', 'visualization'],
            'cloud': ['aws', 'azure', 'gcp'],
            'devops': ['docker', 'kubernetes', 'ci/cd', 'jenkins'],
            'web development': ['react', 'node.js', 'javascript', 'html', 'css'],
            'backend': ['api', 'database', 'sql', 'microservices'],
        }
    
    def calculate_semantic_similarity(
        self, 
        resume_text: str, 
        job_text: str
    ) -> float:
        """Calculate semantic similarity using embeddings"""
        
        if self.semantic_model:
            # Use sentence transformers
            resume_embedding = self.semantic_model.encode(resume_text[:5000])
            job_embedding = self.semantic_model.encode(job_text[:5000])
            
            similarity = cosine_similarity(
                [resume_embedding],
                [job_embedding]
            )[0][0]
        else:
            # Fallback to TF-IDF
            vectorizer = TfidfVectorizer(max_features=500)
            try:
                tfidf_matrix = vectorizer.fit_transform([resume_text, job_text])
                similarity = cosine_similarity(tfidf_matrix[0:1], tfidf_matrix[1:2])[0][0]
            except:
                similarity = 0.5
        
        return float(similarity)
    
    def calculate_skill_match(
        self,
        resume_skills: List[str],
        required_skills: List[str],
        preferred_skills: List[str] = None
    ) -> Tuple[float, Dict]:
        """Calculate skill match with taxonomy awareness"""
        
        if preferred_skills is None:
            preferred_skills = []
        
        # Normalize skills to lowercase
        resume_skills_lower = set(s.lower().strip() for s in resume_skills)
        required_skills_lower = set(s.lower().strip() for s in required_skills)
        preferred_skills_lower = set(s.lower().strip() for s in preferred_skills)
        
        # Direct matches
        direct_required_matches = resume_skills_lower & required_skills_lower
        direct_preferred_matches = resume_skills_lower & preferred_skills_lower
        
        # Related skill matches (using taxonomy)
        related_matches = self._find_related_skills(
            resume_skills_lower,
            required_skills_lower
        )
        
        # Calculate scores
        total_required = len(required_skills_lower)
        total_preferred = len(preferred_skills_lower)
        
        if total_required > 0:
            required_match_rate = (
                len(direct_required_matches) + 
                0.5 * len(related_matches)  # Related skills count as 50%
            ) / total_required
            required_score = min(required_match_rate * 100, 100)
        else:
            required_score = 50.0
        
        if total_preferred > 0:
            preferred_match_rate = len(direct_preferred_matches) / total_preferred
            preferred_score = preferred_match_rate * 100
        else:
            preferred_score = 50.0
        
        # Overall skill score (weighted)
        overall_skill_score = (required_score * 0.8) + (preferred_score * 0.2)
        
        # Missing critical skills
        missing_required = list(required_skills_lower - direct_required_matches - related_matches)
        
        details = {
            'required_score': required_score,
            'preferred_score': preferred_score,
            'direct_matches': list(direct_required_matches),
            'related_matches': list(related_matches),
            'missing_required': missing_required[:10],  # Limit to top 10
            'matched_preferred': list(direct_preferred_matches)
        }
        
        return overall_skill_score, details
    
    def _find_related_skills(
        self,
        candidate_skills: set,
        required_skills: set
    ) -> set:
        """Find skills that are related through taxonomy"""
        related = set()
        
        for req_skill in required_skills:
            # Check if candidate has parent or child skills
            for parent, children in self.skill_taxonomy.items():
                # If required skill is a parent, check for children
                if req_skill == parent:
                    for child in children:
                        if child in candidate_skills:
                            related.add(req_skill)
                            break
                
                # If required skill is a child, check for parent
                if req_skill in children and parent in candidate_skills:
                    related.add(req_skill)
        
        return related
    
    def calculate_experience_match(
        self,
        resume_experience: List[Dict],
        required_years: int = 0,
        required_level: str = "mid-level"
    ) -> Tuple[float, Dict]:
        """Score experience relevance and seniority"""
        
        # Count total years (simplified)
        total_years = len(resume_experience)  # Each entry ~= 1-2 years
        
        # Estimate seniority from titles
        max_seniority = 0
        for exp in resume_experience:
            title = exp.get('title', '').lower()
            for level, weight in self.experience_weights.items():
                if level in title:
                    max_seniority = max(max_seniority, weight)
        
        # Calculate years match
        if required_years > 0:
            years_match = min(total_years / required_years, 1.0) * 100
        else:
            years_match = 75.0 if total_years > 0 else 0.0
        
        # Calculate level match
        required_weight = self.experience_weights.get(required_level, 3)
        level_match = min(max_seniority / required_weight, 1.0) * 100
        
        # Overall experience score
        experience_score = (years_match * 0.6) + (level_match * 0.4)
        
        details = {
            'total_years': total_years,
            'max_seniority': max_seniority,
            'years_match': years_match,
            'level_match': level_match,
            'num_roles': len(resume_experience)
        }
        
        return experience_score, details
    
    def calculate_education_match(
        self,
        resume_education: List[Dict],
        required_degree: str = "",
        required_field: str = ""
    ) -> Tuple[float, Dict]:
        """Score education relevance"""
        
        if not resume_education:
            return 0.0, {'has_education': False}
        
        score = 0
        matched_degree = False
        matched_field = False
        
        for edu in resume_education:
            degree = edu.get('degree', '').lower()
            field = edu.get('field', '').lower()
            
            # Degree level matching
            if required_degree:
                req_deg = required_degree.lower()
                if req_deg in degree:
                    matched_degree = True
                    score += 50
            else:
                # Has any degree
                score += 30
            
            # Field matching
            if required_field and field:
                req_field = required_field.lower()
                if req_field in field or field in req_field:
                    matched_field = True
                    score += 50
        
        # Cap at 100
        score = min(score, 100)
        
        details = {
            'has_education': True,
            'matched_degree': matched_degree,
            'matched_field': matched_field,
            'num_degrees': len(resume_education)
        }
        
        return score, details
    
    def calculate_keyword_density(
        self,
        resume_text: str,
        job_keywords: List[str]
    ) -> float:
        """Calculate how well resume uses job keywords"""
        
        resume_lower = resume_text.lower()
        matches = 0
        
        for keyword in job_keywords:
            if keyword.lower() in resume_lower:
                matches += 1
        
        if job_keywords:
            density = (matches / len(job_keywords)) * 100
        else:
            density = 50.0
        
        return density
    
    def calculate_comprehensive_score(
        self,
        resume_data: Dict,
        job_data: Dict
    ) -> MatchScore:
        """Calculate overall match score with all factors"""
        
        # Extract data
        resume_text = resume_data.get('raw_text', '')
        resume_skills = resume_data.get('skills', [])
        resume_experience = resume_data.get('experience', [])
        resume_education = resume_data.get('education', [])
        
        job_text = job_data.get('raw_text', '')
        required_skills = job_data.get('required_skills', [])
        preferred_skills = job_data.get('preferred_skills', [])
        job_keywords = job_data.get('keywords', [])
        required_years = job_data.get('required_years', 0)
        required_level = job_data.get('required_level', 'mid-level')
        
        # Calculate component scores
        skill_score, skill_details = self.calculate_skill_match(
            resume_skills,
            required_skills,
            preferred_skills
        )
        
        experience_score, exp_details = self.calculate_experience_match(
            resume_experience,
            required_years,
            required_level
        )
        
        education_score, edu_details = self.calculate_education_match(
            resume_education,
            job_data.get('required_degree', ''),
            job_data.get('required_field', '')
        )
        
        semantic_score = self.calculate_semantic_similarity(
            resume_text,
            job_text
        ) * 100
        
        keyword_score = self.calculate_keyword_density(
            resume_text,
            required_skills + job_keywords
        )
        
        # Weighted overall score
        weights = {
            'skills': 0.35,
            'experience': 0.25,
            'education': 0.15,
            'semantic': 0.15,
            'keywords': 0.10
        }
        
        overall_score = (
            skill_score * weights['skills'] +
            experience_score * weights['experience'] +
            education_score * weights['education'] +
            semantic_score * weights['semantic'] +
            keyword_score * weights['keywords']
        )
        
        # Generate recommendation
        recommendation = self._generate_recommendation(
            overall_score,
            skill_details,
            exp_details
        )
        
        # Determine confidence
        confidence = self._calculate_confidence(
            skill_details,
            exp_details,
            edu_details
        )
        
        return MatchScore(
            overall_score=round(overall_score, 1),
            skill_match_score=round(skill_score, 1),
            experience_match_score=round(experience_score, 1),
            education_match_score=round(education_score, 1),
            semantic_similarity=round(semantic_score, 1),
            keyword_density=round(keyword_score, 1),
            missing_critical_skills=skill_details['missing_required'],
            matched_skills=skill_details['direct_matches'],
            preferred_skills_matched=skill_details['matched_preferred'],
            recommendation=recommendation,
            confidence_level=confidence
        )
    
    def _generate_recommendation(
        self,
        score: float,
        skill_details: Dict,
        exp_details: Dict
    ) -> str:
        """Generate hiring recommendation"""
        
        if score >= 85:
            return "Strong Match - Highly Recommended"
        elif score >= 70:
            return "Good Match - Recommended for Interview"
        elif score >= 55:
            return "Moderate Match - Consider with Reservations"
        elif score >= 40:
            return "Weak Match - Not Recommended"
        else:
            return "Poor Match - Reject"
    
    def _calculate_confidence(
        self,
        skill_details: Dict,
        exp_details: Dict,
        edu_details: Dict
    ) -> str:
        """Calculate confidence in the assessment"""
        
        # Check data completeness
        has_skills = len(skill_details['direct_matches']) > 0
        has_experience = exp_details.get('num_roles', 0) > 0
        has_education = edu_details.get('has_education', False)
        
        complete_fields = sum([has_skills, has_experience, has_education])
        
        if complete_fields >= 3:
            return "High Confidence"
        elif complete_fields >= 2:
            return "Medium Confidence"
        else:
            return "Low Confidence - Limited Data"


# Example usage
if __name__ == "__main__":
    scorer = EnhancedScoringEngine(use_transformers=False)
    
    # Sample data
    resume_data = {
        'raw_text': "Experienced Python developer with ML expertise...",
        'skills': ['python', 'tensorflow', 'aws', 'docker'],
        'experience': [
            {'title': 'Senior ML Engineer', 'company': 'Tech Co'},
            {'title': 'Data Scientist', 'company': 'AI Startup'}
        ],
        'education': [
            {'degree': 'Master of Science', 'field': 'Computer Science'}
        ]
    }
    
    job_data = {
        'raw_text': "Looking for ML engineer with Python and cloud experience...",
        'required_skills': ['python', 'machine learning', 'aws'],
        'preferred_skills': ['kubernetes', 'tensorflow'],
        'keywords': ['ml', 'cloud', 'production'],
        'required_years': 3,
        'required_level': 'senior'
    }
    
    score = scorer.calculate_comprehensive_score(resume_data, job_data)
    
    print(f"Overall Score: {score.overall_score}/100")
    print(f"Recommendation: {score.recommendation}")
    print(f"Confidence: {score.confidence_level}")
    print(f"Matched Skills: {score.matched_skills}")
    print(f"Missing Skills: {score.missing_critical_skills}")